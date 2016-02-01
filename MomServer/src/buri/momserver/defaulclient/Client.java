package buri.momserver.defaulclient;

import buri.momserver.core.IClient;
import java.io.IOException;
import java.io.InputStream;
import java.net.Socket;
import java.util.Collection;
import java.util.LinkedList;

/**
 * The server side implementation of the client
 *
 * @author Bela Bursan
 */
public final class Client implements IClient {

    private final LinkedList<String> logger;

    public Client() {
        logger = new LinkedList<>();
    }
    
    private void log(String text, Object ... value){
        logger.addLast(String.format(text, value));
    }

    @Override
    public int execute(final Socket socket) {
        log("running client: %s", Thread.currentThread().getName());

        //this is just test!!!! remove later
        try {
            byte[] buffer = new byte[8196];
            boolean alive = true;
            while (alive) {
                InputStream is = socket.getInputStream();
                is.read(buffer);
                String s = new String(buffer);
                String trim = s.trim();
                System.out.println(trim);
                if (trim.equals("xxx")) {
                    alive = false;
                }
            }
            System.out.println("enddddd");
        } catch (Exception ex) {
            log("Exception: %s", ex.getLocalizedMessage());
        }
        try {
            socket.close();
        } catch (IOException ex) {
            log("IOException: %s", ex.getLocalizedMessage());
        }
        return SUCCESS;
    }

    @Override
    public void close() {
        log("closing cli");
    }

    @Override
    public Collection<String> getLogs() {
        return logger;
    }

}
