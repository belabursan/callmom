package buri.momserver;

/**
 * Holds and parses the command line arguments.
 *
 * The thought here is that flags with single score, like "-p" must have a value
 * and flags with double score, like "--debug" has no following values. Example:
 * ... --debug -p "/some/path/here" --daemon
 *
 * @author Bela Bursan
 */
class CLArguments {

    private String propertyFilePath;
    private boolean debug;
    private boolean daemon;
    //
    private final static String DAEMON = "--daemon";
    private final static String DEBUG = "--debug";
    private final static String PATH = "-p";

    private CLArguments() {
        this.daemon = false;
        this.debug = false;
        this.propertyFilePath = ServerProperties.DEFAULT_PROPERTY_FILE_NAME;
    }

    /**
     * Reads and parses the command line arguments
     *
     * @param args command line arguments to read
     * @return CLArguments object containing the command line arguments or
     * throws exception in case of invalid arguments. If no arguments are set
     * then the default values will be set in the returned object
     * @throws IllegalArgumentException if the input arguments are not correct
     */
    static CLArguments resolveArguments(String[] args) throws IllegalArgumentException {
        CLArguments arguments = new CLArguments();
        boolean next = false;
        String tmpNext = null;
        for (String s : args) {
            if (next && tmpNext != null) {
                if (s.equals("") || s.equals(DAEMON) || s.equals(DEBUG)) {
                    throw new IllegalArgumentException("Invalid command line arguments: flag " + tmpNext + " has no or bad value: " + s);
                }
                switch (tmpNext) {
                    case PATH:
                        arguments.propertyFilePath = s;
                        break;
                }
                next = false;
                tmpNext = null;
            } else {
                switch (s) {
                    case DAEMON:
                        arguments.daemon = true;
                        break;
                    case DEBUG:
                        arguments.debug = true;
                        break;
                    case PATH:
                        next = true;
                        tmpNext = PATH;
                        break;
                    default:
                        throw new IllegalArgumentException("Unrecognized command line argument: " + s);
                }
            }
        }
        if (next) {
            throw new IllegalArgumentException("Invalid command line arguments: flag " + tmpNext + " has no value");
        }
        return arguments;
    }

    public String getPropertyFilePath() {
        return propertyFilePath;
    }

    public boolean isDebug() {
        return debug;
    }

    public boolean isDaemon() {
        return daemon;
    }

}
