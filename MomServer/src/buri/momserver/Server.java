package buri.momserver;

import java.io.File;
import java.io.IOException;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.LinkedBlockingQueue;
import java.util.concurrent.ThreadPoolExecutor;
import java.util.concurrent.TimeUnit;

/**
 *
 * @author Bela Bursan
 */
final class Server extends Thread implements Runnable {

    private static Server server = null;
    private volatile boolean alive;
    private final LinkedBlockingQueue<Runnable> clientQueue;
    private final ExecutorService threadPool;
    private ComModule network;

    private Server(ServerProperties properties) throws IOException {
        alive = false;
        clientQueue = new LinkedBlockingQueue<>();
        threadPool = new ThreadPoolExecutor(3, 200, 60, TimeUnit.SECONDS, clientQueue);
        network = ComModule.getInstance(
                properties.getPort(),
                properties.getNumberOfClients(),
                properties.isReuseAddress()
        );
        network.startTCPServer();
    }

    static Server getInstance(String propertiesPath) throws IOException {
        if (server == null) {
            server = new Server(ServerProperties.readProperties(new File(propertiesPath)));
        }
        return server;
    }

    @Override
    public void run() {
        Thread.currentThread().setName("ServerThread");
        try {
            while (alive) {
                System.out.println("kkkk");
                Thread.sleep(1000);
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
