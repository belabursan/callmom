package buri.momserver;

import java.io.IOException;
import java.net.Socket;

/**
 * Abstract class used as base for the clients
 * @author Bela Bursan
 */
public abstract class IClientTask implements Runnable {

    /* Connected socet, use it to communicate through the network*/
    protected final Socket socket;
    
    protected volatile boolean alive;
    
    /**
     * Constructs a new instance of the IClient object
     * @param socket connected socket to set
     */
    protected IClientTask(Socket socket){
        this.socket = socket;
        alive = true;
    }
    
    /**
     * Returns a new instance of a client
     * @param socket connected socket
     * @return new client object
     */
    public static ClientTask newInstance(Socket socket){
        return new ClientTask(socket);
    }
    
    /**
     * Closes the client, the socket and ends the thread if it is still running
     */
    public void close(){
        alive = false;
        if (this.socket != null) {
            try {
                this.socket.close();
            } catch (IOException ex) {
                //ignore this
            }
        }
    }
}
