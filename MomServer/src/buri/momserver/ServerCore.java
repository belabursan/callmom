package buri.momserver;

import java.io.File;
import java.io.IOException;
import java.util.concurrent.RejectedExecutionHandler;
import java.util.concurrent.SynchronousQueue;
import java.util.concurrent.ThreadPoolExecutor;
import java.util.concurrent.TimeUnit;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 * The server core
 *
 * @author Bela Bursan
 */
final class ServerCore extends Thread implements Runnable {

    private static ServerCore server = null;
    private volatile boolean alive;
    private final ThreadPoolExecutor threadPool;
    private NetworkModule network;
    private static final Logger LOG = Logger.getLogger(MomLogger.LOGGER_NAME);

    /**
     * Creates a new server instance
     *
     * @param properties properties to set and use during execution
     * @throws IOException if the network module cannot be started or the
     * property file cannot be read
     */
    private ServerCore(ServerProperties properties) throws IOException {
        alive = false;
        threadPool = new ThreadPoolExecutor(2, properties.getNumberOfClients() + 1, 60, TimeUnit.SECONDS, new SynchronousQueue<Runnable>());
        threadPool.prestartAllCoreThreads();
        threadPool.setRejectedExecutionHandler(new RejectedExecutionHandler() {
            @Override
            public void rejectedExecution(Runnable r, ThreadPoolExecutor executor) {
                System.out.println("threadpool rejected a runnable");
                System.out.println("Active count: " + executor.getActiveCount());
                if (r instanceof IClientTask) {
                    ((IClientTask) r).close();
                }
                executor.remove(r);
                executor.purge();
            }
        });

        network = NetworkModule.getInstance(
                properties.getPort(),
                properties.getNumberOfClients(),
                properties.isReuseAddress()
        );
        network.startTCPServer();
        LOG.finest("Server core created successfully");
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
    public synchronized void run() {
        Thread.currentThread().setName("ServerThread");
        try {
            while (alive) {
                try {
                    threadPool.execute(IClientTask.newInstance(network.getSocket()));
                } catch (IOException ex) {
                    LOG.log(Level.SEVERE, "Got IO exception when waiting for new client: {0}", ex.getMessage());
                    //failed to get socket, wait 3 seconds and try again
                    wait(2000);
                }
            }
        } catch (InterruptedException ix) {
            LOG.finest("ServerThread Interrupted");
        }
        //System.out.println("ServerThread ended");
    }

    /**
     * Starts the thread of the server If already started the call to this
     * method has effect
     *
     * @return true if the server is alive after this call, false otherwise
     */
    boolean startServer() {
        LOG.finest("starting to run core");
        if (this.getState() == State.NEW) {
            alive = true;
            this.start();
        }
        return alive;
    }

    /**
     * Stops all client execution and stops and waits for the server thread
     */
    void closeServer() {
        alive = false;
        LOG.info(" -closing MomServer");

        if (network != null) {
            network.close();
            network = null;
        }
        if (!threadPool.isTerminated()) {
            threadPool.shutdown();
            try {
                if (!threadPool.awaitTermination(5, TimeUnit.SECONDS)) {
                    threadPool.shutdownNow();
                }
            } catch (InterruptedException ix) {
                LOG.warning("threadpool interrupted when closing server core");
                //just ignore
            }
        }
        
        try {
            this.interrupt();
            this.join();
        } catch (InterruptedException iix) {
            LOG.warning("join() interrupted when closing server core");
            //just ignore
        }
        //TODO close other things of the server if needed
    }
}
