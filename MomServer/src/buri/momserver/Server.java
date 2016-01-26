package buri.momserver;

import java.io.File;
import java.io.IOException;
import java.util.Properties;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.LinkedBlockingQueue;
import java.util.concurrent.ThreadPoolExecutor;
import java.util.concurrent.TimeUnit;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 *
 * @author Bela Bursan
 */
class Server extends Thread implements Runnable {

    private static Server server = null;
    private volatile boolean alive;
    private final LinkedBlockingQueue<Runnable> clientQueue;
    private final ExecutorService threadPool;
    private final ComModule network;
    

    private Server(ServerProperties properties) {
        alive = false;
        clientQueue = new LinkedBlockingQueue<>();
        threadPool = new ThreadPoolExecutor(3, 200, 60, TimeUnit.SECONDS, clientQueue);
        network = new ComModule(
                properties.getPort(),
                properties.getNumberOfClients(),
                properties.isReuseAddress()
        );
    }

    static Server getInstance(String propertiesPath) throws IOException{
        if (server == null) {
            server = new Server(ServerProperties.readProperties(new File(propertiesPath)));
        }
        return server;
    }

    @Override
    public void run() {
        try {
            ServerProperties.createDefaultPropertyFile();
            /*
            try {
            while (alive) {

            }
            } catch (InterruptedException ix) {
            
            }
            */
        } catch (IOException ex) {
            Logger.getLogger(Server.class.getName()).log(Level.SEVERE, null, ex);
        }
    }

    /**
     * Starts the thread of the server If already started the call to this
     * method has effect
     *
     * @return true if the server is alive after this call, false otherwise
     */
    boolean startServer() {
        if (this.getState() == State.NEW) {
            alive = true;
            this.start();
        }
        return alive;
    }

    /**
     * Stops all client execution and stops and waits for the server thread
     *
     * @throws InterruptedException in case of the calling thread is interrupted
     * during this call
     */
    void closeServer() throws InterruptedException {
        alive = false;
        threadPool.shutdown();
        try {
            if (!threadPool.awaitTermination(5, TimeUnit.SECONDS)) {
                threadPool.shutdownNow();
            }
        } catch (InterruptedException ix) {
            //should never occur
        }
        this.join();
        //TODO close other things of the server if needed
    }
}
