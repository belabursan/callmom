package buri.momserver.defaulclient;

import buri.momserver.core.IClient;
import java.io.IOException;
import java.net.Socket;
import java.util.Collection;
import java.util.LinkedList;

/**
 * The server side implementation of the client
 *
 * @author Bela Bursan
 */
public final class Client implements IClient {

    private final int timeout = 1000;
    private final LinkedList<String> logger;
    private Com com;
    private Socket socket;

    public Client() {
        logger = new LinkedList<>();
    }

    private void log(String text, Object... value) {
        logger.addLast(String.format(text, value));
    }

    @Override
    public int execute(final Socket socket) {
        this.socket = socket;
        int retValue = SUCCESS;
        boolean alive = true;
        TLV response = null;

        log("running client: %s", Thread.currentThread().getName());

        try {
            //create new communication module
            this.com = new Com(this.socket);

            while (alive) {

                //read a TLV from the network
                TLV message = com.read(timeout);

                //check if protocol is valid
                if (Protocol.isValidMessage(message, Protocol.PROTOCOL_VERSION)) {
                    log("message is not valid, bad version? (%s)", Protocol.PROTOCOL_VERSION);
                    response = Protocol.createResponse(Protocol.RESPONSE_505, "Bad version");
                    retValue = FAILURE;
                    break;
                }

                //parse command and execute or fail
                String command = Protocol.getCommand(message);
                switch (command) {
                    case Protocol.REQUEST_BLINK:
                        Collection<String> args = Protocol.getArguments(message);
                        String resp = blink(args);
                        break;
                    case Protocol.REQUEST_EXIT:
                        response = Protocol.createResponse(Protocol.RESPONSE_200, null);
                        alive = false;
                        break;

                    default:
                        log("Command not recognized : [%s]", command);
                        response = Protocol.createResponse(Protocol.RESPONSE_501, "Command not recognized");
                        retValue = FAILURE;
                        //stop execution
                        alive = false;
                        break;
                }
            }
        } catch (Exception ex) {
            retValue = FAILURE;
            log("exception when executing client: %s", ex.getLocalizedMessage());
            response = Protocol.createResponse(Protocol.RESPONSE_500, ex.getLocalizedMessage());
        } finally {

            if (socket != null) {
                //send response
                if (response != null) {
                    com.send(response);
                }

                //close socket
                try {
                    socket.close();
                } catch (IOException ex) {
                    log("IOexception when closing socket");
                }
            }
        }

        return retValue;
    }

    @Override
    public void close() {
        log("closing client");
        //TODO close
    }

    @Override
    public Collection<String> getLogs() {
        return logger;
    }

    private String blink(Collection<String> args) {
        throw new UnsupportedOperationException("Not supported yet."); //To change body of generated methods, choose Tools | Templates.
    }

}
