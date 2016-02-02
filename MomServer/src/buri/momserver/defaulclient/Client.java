package buri.momserver.defaulclient;

import buri.momserver.core.IClient;
import java.io.IOException;
import java.net.Socket;
import java.net.SocketTimeoutException;
import java.util.Collection;
import java.util.LinkedList;

/**
 * The server side implementation of the client
 *
 * @author Bela Bursan
 */
public final class Client implements IClient {

    private final int timeout = 10000;
    private final LinkedList<String> logger;
    private Com com;
    private Socket socket;
    private boolean isDebug;

    public Client() {
        logger = new LinkedList<>();
    }

    private void log(String text, Object... value) {
        logger.addLast(String.format(text, value));
    }

    @Override
    public int execute(Socket s, boolean debug) {
        this.socket = s;
        this.isDebug = debug;
        int retValue = FAILURE;
        boolean alive = true;
        TLV response = null;

        log("running client: %s", Thread.currentThread().getName());

        try {
            //create new communication module
            this.com = new Com(this.socket, this.isDebug, this.logger);

            while (alive) {

                //read a TLV from the network
                TLV message = com.read(timeout);

                //check if protocol is valid
                if (Protocol.isValidMessage(message, Protocol.PROTOCOL_VERSION)) {
                    log("message is not valid, bad version? (%s)", Protocol.PROTOCOL_VERSION);
                    response = Protocol.createResponse(Protocol.RESPONSE_505, "Bad version");
                    break;
                }

                //parse command and execute or fail
                String command = Protocol.getCommand(message);
                switch (command) {
                    case Protocol.REQUEST_BLINK:
                        Collection<String> args = Protocol.getArguments(message);
                        String resp = blink(args);
                        retValue = SUCCESS;
                        break;

                    case Protocol.REQUEST_EXIT:
                        log("got exit command, finishing...");
                        response = Protocol.createResponse(Protocol.RESPONSE_200, null);
                        alive = false;
                        retValue = SUCCESS;
                        break;

                    default:
                        log("Command not recognized : [%s]", command);
                        response = Protocol.createResponse(Protocol.RESPONSE_501, "Command not recognized");
                        //stop execution
                        alive = false;
                        break;
                }
            }
        } catch (IllegalArgumentException ix) {
            log("illegal arg exception when executing client: %s", ix.getLocalizedMessage());
            response = Protocol.createResponse(Protocol.RESPONSE_400, ix.getLocalizedMessage());
        } catch (SocketTimeoutException tx) {
            log("timeout exception when executing client: %s", tx.getLocalizedMessage());
            response = Protocol.createResponse(Protocol.RESPONSE_408, "Timed out after " + timeout + " milliseconds. ");
        } catch (IOException ex) {
            log("io exception when executing client: %s", ex.getLocalizedMessage());
            response = null; //no response, the network is down
        } catch (Exception ex) {
            //catch all other exception
            log("Exception when executing client: %s", ex.getLocalizedMessage());
            response = Protocol.createResponse(Protocol.RESPONSE_500, ex.getLocalizedMessage());
        } finally {
            sendResponse(response);
            close();
        }

        return retValue;
    }

    /**
     * Sends a response to the connected client
     *
     * @param response TLV object to send
     */
    private void sendResponse(TLV response) {
        //send response
        if (response != null) {
            try {
                com.send(response);
            } catch (IOException ex) {
                log("Exception when sending response: " + ex.getLocalizedMessage());
            }
        }
    }

    @Override
    public synchronized void close() {
        log("closing client");
        if (com != null) {
            com.close();
            com = null;
        }
        if (socket != null) {
            try {
                socket.shutdownInput();
                socket.close();
            } catch (IOException ex) {
                log("IOEx when closing socket: %s", ex.getLocalizedMessage());
            }
            this.socket = null;
        }
    }

    @Override
    public Collection<String> getLogs() {
        return logger;
    }

    /**
     * This method calls the Tell-stick and makes a lamp to blink
     *
     * @param args list of arguments
     * @return
     */
    private String blink(Collection<String> args) {
        System.out.println("handeling blink");
        return "";
    }

}
