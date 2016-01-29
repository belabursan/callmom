package buri.momserver;

import java.io.IOException;
import java.io.InputStream;
import java.net.Socket;
import java.util.Collection;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 * The server side implementation of the client
 *
 * @author Bela Bursan
 */
final class Client implements IClient {

    private final static Logger LOG = Logger.getLogger(MomLogger.LOGGER_NAME);

    Client() {
    }

    @Override
    public int main(final Socket socket) {
        LOG.log(Level.FINEST, "running client: {0}", Thread.currentThread().getName());

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
            LOG.log(Level.SEVERE, null, ex);
        }
        try {
            socket.close();
        } catch (IOException ex) {
            LOG.log(Level.SEVERE, null, ex);
        }
        return SUCCESS;
    }

    @Override
    public void close() {
        System.out.println("closing cli");
    }

    @Override
    public Collection getLogs() {
        //not implemented in this implementation since we have access to the real logger
        return null;
    }

}
