package buri.momserver;

import java.io.File;
import java.io.IOException;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.LinkedBlockingQueue;
import java.util.concurrent.ThreadPoolExecutor;
import java.util.concurrent.TimeUnit;

/**
 * The server core
 *
 * @author Bela Bursan
 */
final class ServerCore extends Thread implements Runnable {

    private static ServerCore server = null;
    private volatile boolean alive;
    private final LinkedBlockingQueue<Runnable> clientQueue;
    private final ExecutorService threadPool;
    private NetworkModule network;

    /**
     * Creates a new server instance
     *
     * @param properties properties to set and use during execution
     * @throws IOException if the network module cannot be started or the
     * property file cannot be read
     */
    private ServerCore(ServerProperties properties) throws IOException {
        alive = false;
        clientQueue = new LinkedBlockingQueue<>();
        threadPool = new ThreadPoolExecutor(3, 200, 60, TimeUnit.SECONDS, clientQueue);
        network = NetworkModule.getInstance(
                properties.getPort(),
                properties.getNumberOfClients(),
                properties.isReuseAddress()
        );
        network.startTCPServer();
    }

    /**
     * Returns a singleton instance of the server core
     *
     * @param propertiesPath path to property file
     * @return the only instance of the ServerCore object
     * @throws IOException if the property file cannot be read
     */
    static ServerCore getInstance(String propertiesPath) throws IOException {
        if (server == null) {
            server = new ServerCore(ServerProperties.readProperties(new File(propertiesPath)));
        }
        return server;
    }

    /**
     * Method where the server thread is run
     */
    @Override
    public void run() {
        Thread.currentThread().setName("ServerThread");
        try {
            while (alive) {
                System.out.println("kkkk");
                Thread.sleep(1000);//remove later, only test code
            }
        } catch (InterruptedException ix) {

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
        if (network != null) {
            network.close();
            network = null;
        }
        this.join();
        //TODO close other things of the server if needed
    }
}
