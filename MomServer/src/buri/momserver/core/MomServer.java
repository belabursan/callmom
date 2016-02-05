package buri.momserver.core;

import java.io.File;
import java.io.IOException;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 * MomServer main class
 *
 * @author Bela Bursan
 */
public final class MomServer {

    static final String VERSION = "1.0.0";
    private ServerCore serverCore;
    private Thread shutDownThread;
    private static final Logger LOG = Logger.getLogger(MomLogger.LOGGER_NAME);

    /**
     * Signals the wait in the run() method. Also closes the server core. Only
     * called by the shutdown hook!
     */
    private synchronized void shutDown() {
        LOG.warning("got shutdown signal");
        if (serverCore != null) {
            serverCore.closeServer();
            serverCore = null;
        }
        notifyAll();
    }

    /**
     * Starts and runs the server, blocks until a shutdown is initialized and
     * then closes the server.
     *
     * @param args command line arguments, holds an alternative path to the
     * property file
     * @throws InterruptedException if interrupted during the close process
     */
    private synchronized void execute(
            final CLArguments arguments,
            final ServerProperties properties) throws InterruptedException {
        registerShutDownHook();
        try {
            LOG.finest("starting to execute server");

            //create and start the server core
            serverCore = ServerCore.getInstance(properties);
            if (serverCore.startServer()) {
                if (arguments.isDaemon() && !arguments.isDebug()) {
                    LOG.warning("server will run as daemon");

                    //TODO check this later...
                    File pidfile = new File(System.getProperty("daemon.pidfile"));
                    pidfile.deleteOnExit();
                    System.out.close();
                    System.err.close();
                } else{
                    System.out.println(" - server started, exit with Ctrl+C");
                }
                //wait until the shut down hook is activated
                wait();
            }
        } catch (InterruptedException ix) {
            LOG.log(Level.SEVERE, "Intrrupted exception: {0}", ix.getMessage());
        } catch (IOException ex) {
            LOG.log(Level.SEVERE, "IO exception: {0}", ex.getMessage());
        } finally {
            awaitShutdownHook();
        }
    }

    /**
     * Awaits the termination of the shut down thread and unregisters it.
     *
     * @throws InterruptedException
     */
    private void awaitShutdownHook() throws InterruptedException {
        LOG.info("awaiting shut down hook");
        if (shutDownThread.getState() != Thread.State.NEW) {
            LOG.finest("shutdown hook startat, now trying to join");
            shutDownThread.join();
        }
    }

    /**
     * Creates a shutdown hook the hook will call the shutDown() method at
     * shutdown
     */
    private void registerShutDownHook() {
        LOG.info("registering shut down hook");

        shutDownThread = new Thread() {
            @Override
            public void run() {
                Thread.currentThread().setName("ShutDownThread");
                shutDown();
            }
        };
        //add shutdown hook
        Runtime.getRuntime().addShutdownHook(shutDownThread);
    }

    /**
     * Main method, everything starts and ends here
     *
     * @param args the command line arguments
     */
    public static void main(final String[] args) {
        Thread.currentThread().setName("MainThread");
        
        try {
            //parse the command line arguments
            CLArguments arguments = CLArguments.resolveArguments(args);

            if(!arguments.isDaemon()){
                System.out.println("\nStarting MomServer v" + VERSION);
            }
            //read properties from file
            ServerProperties properties = ServerProperties.readProperties(
                    new File(arguments.getPropertyFilePath()));

            //init logger
            MomLogger.initLogger(
                    (arguments.isDebug() || properties.isDebug()) ? Level.FINEST : Level.WARNING);
            LOG.severe(" --== Starting MomServer v" + VERSION + " ==--");

            //create and run
            final MomServer mom = new MomServer();
            mom.execute(arguments, properties);
        } catch (InterruptedException ex) {
            LOG.log(Level.SEVERE, "Main thread interrupted, exiting! ({0})", ex.getMessage());
        } catch (IllegalArgumentException ix) {
            LOG.log(Level.SEVERE, "Failed to start the server: {0}", ix.getMessage());
        } catch (SecurityException | IOException ex) {
            LOG.log(Level.SEVERE, "Could not start logger: {0}", ex.getMessage());
        } finally {
            MomLogger.closeLogger();
        }
        System.exit(0);
    }
}
