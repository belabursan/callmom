package buri.momserver.core;

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

    public static final String LOGGER_NAME = "buri.momserver.momlogger";
    private static final String LOG_FILE_NAME = "momservlog_%g.html";
    //
    private static final Logger LOGGER = Logger.getLogger(LOGGER_NAME);
    private static FileHandler fileHandler = null;
    //
    private static String logDirectoryPath = "logs";
    private static Level logLevel = Level.WARNING;
    private static int logFileSize = 500000;
    private static int noofLogFiles = 10;

    /**
     * Initializes the logger with values.
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
        MomLogger.logDirectoryPath = logDirPath;
        MomLogger.logLevel = logLevel;
        MomLogger.logFileSize = logFileSize;
        MomLogger.noofLogFiles = noofLogFiles;
        MomLogger.initLogger();
    }

    /**
     * Initializes the logger specifying the log level value.
     *
     * @param logLevel log level to set
     * @throws SecurityException
     * @throws IOException
     */
    static void initLogger(Level logLevel) throws SecurityException, IOException {
        MomLogger.logLevel = logLevel;
        MomLogger.initLogger();
    }

    /**
     * Initializes the logger with default values
     *
     * @throws SecurityException
     * @throws IOException
     */
    static void initLogger() throws SecurityException, IOException {
        MomLogger.startLogger();
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
    }

    /**
     * Starts the server myLogger. The myLogger should only be used to log
     * server events and not client events
     *
     * @throws SecurityException
     * @throws IOException
     */
    private static void startLogger() throws SecurityException, IOException {
        final File logdir = new File(logDirectoryPath);
        if (logdir.exists()) {
            if (!logdir.isDirectory()) {
                logdir.mkdir();
            }
        } else {
            logdir.mkdir();
        }
        if (!logdir.exists()) {
            throw new IOException("startLogger() - could not create logdirectory: "
                    + logdir.getAbsolutePath());
        }

        final Handler console = new ConsoleHandler();
        LOGGER.addHandler(console);
        console.setLevel(Level.OFF);
        fileHandler = new FileHandler(
                logdir.getAbsolutePath() + File.separator + LOG_FILE_NAME,
                logFileSize, noofLogFiles,
                true);
        fileHandler.setLevel(logLevel);
        fileHandler.setFormatter(new MomHtmlFormatter());
        LOGGER.addHandler(fileHandler);
        LOGGER.setLevel(logLevel);
        LOGGER.setUseParentHandlers(false);
    }

    /**
     * HTML formatter used by the logger
     */
    private final static class MomHtmlFormatter extends Formatter {

        private static volatile boolean isDark = true;
        private static volatile long lineNumber = 0;
        private static final SimpleDateFormat DATE_FORMATTER = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss_SSS");
        private static final StringBuilder BUFFER = new StringBuilder(1024);

        @Override
        public String format(final LogRecord rec) {
            BUFFER.replace(0, BUFFER.length(), "");
            lineNumber++;
            isDark = !isDark;
            if (isDark) {
                BUFFER.append("\t\t\t\t<tr class=\"d\">");
            } else {
                BUFFER.append("\t\t\t\t<tr>");
            }
            BUFFER.append("<td>");
            BUFFER.append(lineNumber);
            BUFFER.append("</td>");
            if (rec.getLevel().intValue() >= Level.SEVERE.intValue()) {
                BUFFER.append("<td class=\"s\">");
            } else if (rec.getLevel().intValue() >= Level.WARNING.intValue()) {
                BUFFER.append("<td class=\"w\">");
            } else if (rec.getLevel().intValue() >= Level.INFO.intValue()) {
                BUFFER.append("<td class=\"i\">");
            } else {
                BUFFER.append("<td class=\"f\">");
            }
            BUFFER.append(rec.getLevel());
            BUFFER.append("</td><td>");
            BUFFER.append(calculateDate(rec.getMillis()));
            BUFFER.append("</td><td>");
            BUFFER.append(rec.getSourceClassName());
            BUFFER.append(":");
            BUFFER.append(rec.getSourceMethodName());
            BUFFER.append("</td><td>");
            BUFFER.append(formatMessage(rec));
            BUFFER.append("</td></tr>\n");
            BUFFER.trimToSize();
            return BUFFER.toString();
        }

        private String calculateDate(final long millisecs) {
            return DATE_FORMATTER.format(new Date(millisecs));
        }

        @Override
        public String getHead(final Handler h) {
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
        public String getTail(final Handler h) {
            lineNumber = 0;
            isDark = false;
            return "\t\t\t</table>\n\t\t</PRE>\n\t</BODY>\n</HTML>\n";
        }
    }

}
