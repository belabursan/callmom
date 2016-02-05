package buri.momserver.core;

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
    private final ServerProperties properties;

    /**
     * Creates a new server instance
     *
     * @param properties properties to set and use during execution
     * @throws IOException if the network module cannot be started or the
     * property file cannot be read
     */
    private ServerCore(ServerProperties properties) throws IOException {
        alive = false;
        this.properties = properties;
        //create and prestart threadpool
        threadPool = new ThreadPoolExecutor(2, properties.getNumberOfClients() + 1, 60, TimeUnit.SECONDS, new SynchronousQueue<Runnable>());
        threadPool.prestartAllCoreThreads();
        threadPool.setRejectedExecutionHandler(new RejectedExecutionHandler() {
            @Override
            public void rejectedExecution(Runnable r, ThreadPoolExecutor executor) {
                Logger.getLogger(MomLogger.LOGGER_NAME).severe("threadpool rejected a runnable");
                Logger.getLogger(MomLogger.LOGGER_NAME).log(Level.SEVERE, "Active count: {0}", executor.getActiveCount());
                if (r instanceof IClient) {
                    ((IClient) r).close();
                }
                executor.remove(r);
                executor.purge();
            }
        });

        //create and start the network module which listens for new connections
        network = NetworkModule.getInstance(
                properties.getPort(),
                properties.getNumberOfClients(),
                properties.isReuseAddress()
        );
        if (properties.isSSL()) {
            network.startSSLServer();
        } else {
            network.startTCPServer();
        }
        LOG.finest("Server core created successfully");
    }

    /**
     * Returns a singleton instance of the server core
     *
     * @param properties property file
     * @return the only instance of the ServerCore object
     * @throws IOException if the property file cannot be read
     */
    static ServerCore getInstance(ServerProperties properties) throws IOException {
        if (server == null) {
            server = new ServerCore(properties);
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
            LOG.fine("waiting for clients to connect...");
            while (alive) {
                try {
                    threadPool.execute(ClientTask.newInstance(network.getSocket(), properties.isDebug()));
                } catch (IOException ex) {
                    LOG.log(Level.SEVERE, "Got IO exception when waiting for new client: {0}", ex.getMessage());
                    //failed to get socket, wait 3 seconds and try again
                    wait(2000);
                }
            }
        } catch (InterruptedException ix) {
            //the server thread is interrupted
            LOG.log(Level.FINEST, "ServerThread Interrupted: {0}", ix.getMessage());
        }
        LOG.finest("server core thread ended");
    }

    /**
     * Starts the thread of the server If already started the call to this
     * method has effect
     *
     * @return true if the server is alive after this call, false otherwise
     */
    boolean startServer() {
        LOG.finest("start to run core");
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
        LOG.info("closing MomServer");

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
            //interrupt the server thread and join it before returning
            this.interrupt();
            this.join();
        } catch (InterruptedException iix) {
            LOG.warning("join() interrupted when closing server core");
            //just ignore
        }
        //TODO close other things of the server if needed
    }
}
