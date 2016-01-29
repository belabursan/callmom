package buri.momserver;

import java.io.File;
import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.logging.ConsoleHandler;
import java.util.logging.FileHandler;
import java.util.logging.Formatter;
import java.util.logging.Handler;
import java.util.logging.Level;
import java.util.logging.LogRecord;
import java.util.logging.Logger;

/**
 * Mom server logger, writes logs to a file
 *
 * @author Bela Bursan
 */
final class MomLogger {

    private static MomLogger momLogger = null;
    //
    private final String logDirectoryPath;
    private final Level logLevel;
    private final int logFileSize;
    private final int noofLogFiles;
    //
    private static final String DEFAULT_LOGDIR_PATH = "logs";
    private static final int DEFAULT_LOGFILE_SIZE = 500000;
    private static final int DEFAULT_NOOF_FILES = 10;
    private static final String LOG_FILE_NAME = "momservlog_%g.html";
    //
    private static final Logger LOGGER = Logger.getLogger("buri.momserver");
    private static FileHandler fileHandler = null;

    /**
     * Creates a logger with default values
     */
    private MomLogger() {
        logDirectoryPath = DEFAULT_LOGDIR_PATH;
        logLevel = Level.WARNING;
        logFileSize = DEFAULT_LOGFILE_SIZE;
        noofLogFiles = DEFAULT_NOOF_FILES;
    }

    private MomLogger(
            String logDirPath,
            Level logLevel,
            int logFileSize,
            int noofLogFiles) {
        this.logDirectoryPath = logDirPath;
        this.logLevel = logLevel;
        this.logFileSize = logFileSize;
        this.noofLogFiles = noofLogFiles;
    }

    /**
     * Initializes the logger with values. If the logger is already initialized
     * this call has no effect
     *
     * @param logDirPath alternative log directory path
     * @param logLevel log level to use
     * @param logFileSize the size of each log file
     * @param noofLogFiles number of files to save
     * @throws SecurityException
     * @throws IOException
     */
    static void initLogger(
            String logDirPath,
            Level logLevel,
            int logFileSize,
            int noofLogFiles) throws SecurityException, IOException {
        if (momLogger == null) {
            momLogger = new MomLogger(logDirPath, logLevel, logFileSize, noofLogFiles);
            momLogger.startLogger();
        }

    }

    /**
     * Initializes the logger with values. If the logger is already initialized
     * this call has no effect
     *
     * @param logLevel log level to set
     * @throws SecurityException
     * @throws IOException
     */
    static void initLogger(Level logLevel) throws SecurityException, IOException {
        initLogger(DEFAULT_LOGDIR_PATH, logLevel, DEFAULT_LOGFILE_SIZE, DEFAULT_NOOF_FILES);
    }

    /**
     * Returns the logger. initializes it if needed
     *
     * @return the logger
     * @throws SecurityException
     * @throws IOException
     */
    static MomLogger getLogger() throws SecurityException, IOException {
        if (momLogger == null) {
            momLogger = new MomLogger();
            momLogger.startLogger();
        }
        return momLogger;
    }

    /**
     * Closes the logger, removes the file handler
     */
    static void closeLogger() {
        LOGGER.removeHandler(fileHandler);
        if (fileHandler != null) {
            fileHandler.flush();
            try {
                fileHandler.close();
            } catch (Exception nu) {
                //do nothing
            }
            fileHandler = null;
        }
        MomLogger.momLogger = null;
    }

    /**
     * Starts the server myLogger. The myLogger should only be used to log
     * server events and not client events
     *
     * @throws SecurityException
     * @throws IOException
     */
    private void startLogger() throws SecurityException, IOException {
        File logdir = new File(logDirectoryPath);
        if (logdir.exists()) {
            if (!logdir.isDirectory()) {
                logdir.mkdir();
            }
        } else {
            logdir.mkdir();
        }
        if (!logdir.exists()) {
            throw new IOException("startLogger() - could not create logdirectory: " + logdir.getAbsolutePath());
        }

        Handler console = new ConsoleHandler();
        LOGGER.addHandler(console);
        console.setLevel(Level.OFF);
        fileHandler = new FileHandler(logdir.getAbsolutePath() + File.separator + LOG_FILE_NAME,
                logFileSize, noofLogFiles, true);
        fileHandler.setLevel(logLevel);
        fileHandler.setFormatter(new HtmlFormatter());
        LOGGER.addHandler(fileHandler);
        LOGGER.setLevel(logLevel);
        LOGGER.setUseParentHandlers(false);
    }

    /**
     * Writes a line to the log file with DEBUG(finest) level
     *
     * @param text text to write
     */
    void debug(String text) {
        LOGGER.finest(text);
    }

    /**
     * Writes a line to the log file with INFO(fine) level
     *
     * @param text text to write
     */
    void info(String text) {
        LOGGER.fine(text);
    }

    /**
     * Writes a line to the log file with WARNING(warning) level
     *
     * @param text text to write
     */
    void warning(String text) {
        LOGGER.warning(text);
    }

    /**
     * Writes a line to the log file with ERROR(severe) level
     *
     * @param text text to write
     */
    void error(String text) {
        LOGGER.severe(text);
    }

    private final class HtmlFormatter extends Formatter {

        private volatile boolean isDark = true;
        private volatile long lineNumber = 0;
        private final SimpleDateFormat date_format = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss_SSS");
        StringBuilder buffer = new StringBuilder(1024);

        @Override
        public String format(LogRecord rec) {
            buffer.replace(0, buffer.length(), "");
            lineNumber++;
            isDark = !isDark;
            if (isDark) {
                buffer.append("\t\t\t\t<tr class=\"d\">");
            } else {
                buffer.append("\t\t\t\t<tr>");
            }
            buffer.append("<td>");
            buffer.append(lineNumber);
            buffer.append("</td>");
            if (rec.getLevel().intValue() >= Level.SEVERE.intValue()) {
                buffer.append("<td class=\"s\">");
            } else if (rec.getLevel().intValue() >= Level.WARNING.intValue()) {
                buffer.append("<td class=\"w\">");
            } else if (rec.getLevel().intValue() >= Level.INFO.intValue()) {
                buffer.append("<td class=\"i\">");
            } else {
                buffer.append("<td class=\"f\">");
            }
            buffer.append(rec.getLevel());
            buffer.append("</td><td>");
            buffer.append(calculateDate(rec.getMillis()));
            buffer.append("</td><td>");
            buffer.append(rec.getSourceClassName());
            buffer.append(":");
            buffer.append(rec.getSourceMethodName());
            buffer.append("</td><td>");
            buffer.append(formatMessage(rec));
            buffer.append("</td></tr>\n");
            buffer.trimToSize();
            return buffer.toString();
        }

        private String calculateDate(long millisecs) {
            return date_format.format(new Date(millisecs));
        }

        @Override
        public String getHead(Handler h) {
            return "<HTML>\n\t<HEAD>\n\t\t<title>MomServer Logs</title>\n\t\t<!-- for help: burszan@gmail.com  -->"
                    + "\n\t\t<style type=\"text/css\">\n\t\t\ttd{padding-right:10px;\n\t\t\t   "
                    + "padding-left:10px;}\n\t\t\ttr.d{background-color:#E6E6E6;}"
                    + "\n\t\t\ttr.h{background-color:#696969;}\n\t\t\t"
                    + "td.i{background-color:#87CEFA;}\n\t\t\ttd.w{background-color:#FF8C00;}"
                    + "\n\t\t\ttd.s{background-color:#FF0000;}"
                    + "\n\t\t\ttd.f{background-color:#FFFFFF;}\n\t\t\tbody{margin: 20px;}"
                    + "\n\t\t</style>\n\t</HEAD>\n\t<BODY>\n\t\t<h2>MomServer - " 
                    + (new Date()) + "</h2>\n\t\t<PRE>\n\t\t\t<table>"
                    + "\n\t\t\t\t<tr class=\"h\"><th>LINE</th><th>LEVEL</th><th>TIME</th>"
                    + "<th>Class:Method</th><th>Log Message</th></tr>\n";
        }

        @Override
        public String getTail(Handler h) {
            lineNumber = 0;
            isDark = false;
            return "\t\t\t</table>\n\t\t</PRE>\n\t</BODY>\n</HTML>\n";
        }
    }

}
