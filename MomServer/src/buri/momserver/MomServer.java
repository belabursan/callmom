package buri.momserver;

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

    static final String VERSION = "0.0.6";
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
    private synchronized void execute(final CLArguments arguments) throws InterruptedException {
        registerShutDownHook();
        try {
            LOG.finest("starting to execute server");

            //create and start the server core
            serverCore = ServerCore.getInstance(arguments.getPropertyFilePath());
            if (serverCore.startServer()) {
                System.out.println("MomServer v" + VERSION + " started, exit with Ctrl+C");

                if (arguments.isDaemon()) {
                    LOG.warning("server will run as daemon");

                    //TODO check this later...
                    File pidfile = new File(System.getProperty("daemon.pidfile"));
                    pidfile.deleteOnExit();
                    System.out.close();
                    System.err.close();
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
        System.out.println("");
        try {
            //parse the command line arguments
            CLArguments arguments = CLArguments.resolveArguments(args);

            //init logger
            MomLogger.initLogger(arguments.isDebug() ? Level.FINEST : Level.WARNING);
            LOG.severe("Starting MomServer v" + VERSION);

            //create and run
            final MomServer mom = new MomServer();
            mom.execute(arguments);
        } catch (InterruptedException ex) {
            System.out.println("Main thread interrupted, exiting! (" + ex.getMessage() + ")");
        } catch (IllegalArgumentException ix) {
            System.out.println("Failed to start the server: " + ix.getMessage());
        } catch (SecurityException | IOException ex) {
            System.out.println("Could not start logger: " + ex.getMessage());
        } finally {
            MomLogger.closeLogger();
        }
        System.exit(0);
    }
}
