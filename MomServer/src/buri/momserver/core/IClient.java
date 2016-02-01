package buri.momserver.core;

import java.net.Socket;
import java.util.Collection;

/**
 * Interface used by the client implementations
 *
 * @author Bela Bursan
 */
public interface IClient {

    //error codes returned by the client
    public static final int SUCCESS = 0;
    public static final int FAILURE = 9999;

    /**
     * Main method for the client, execution starts here
     *
     * @param socket connected socket
     * @return error code, see constants
     */
    public int execute(final Socket socket);

    /**
     * Closes the client. This method will be called by the server when the call
 to execute() returns
     */
    public void close();

    /**
     * Returns the logs of the client. This is only called if the return value
 of the execute method is not success or it throws an exception
     *
     * @return a collection of strings where every element corresponds to a log
     * line
     */
    public Collection<String> getLogs();
}
