package buri.momserver.defaulclient;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.LinkedList;

/**
 * Performs commands against the TellStick
 *
 * @author Bela Bursan
 */
final class TellStick {

    private static TellStick tellStick;
    private final LinkedList<String> logger;
    //
    private static final String COMMAND_ON = "tdtool --on ";
    private static final String COMMAND_OFF = "tdtool --off ";

    private TellStick(LinkedList<String> logger) {
        this.logger = logger;
    }

    /**
     * Adds a line to the logger
     * @param text text to add
     */
    private void log(String text) {
        this.logger.add(text);
    }

    /**
     * Returns a TellStick object
     *
     * @param logger logger
     * @return single instance of TellStick object
     */
    static TellStick getInstance(LinkedList<String> logger) {
        if (tellStick == null) {
            tellStick = new TellStick(logger);
        }
        return tellStick;
    }

    /**
     * Executes a command on the shell
     *
     * @param command command to execute
     * @return the answer returned by the shell
     */
    private String executeCommand(String command) {
        BufferedReader reader = null;
        StringBuilder output = new StringBuilder();
        log("executing command: " + command);

        try {
            Process p = Runtime.getRuntime().exec(command);
            p.waitFor();

            reader = new BufferedReader(new InputStreamReader(p.getInputStream()));

            log("reading response from command");
            String line;
            while ((line = reader.readLine()) != null) {
                output.append(line).append("\n");
            }

        } catch (IOException | InterruptedException e) {
            log("Exception when executing command: " + e.getLocalizedMessage());
        } finally {
            if (reader != null) {
                try {
                    reader.close();
                } catch (IOException ex) {
                    log("exception when closing reader in tellstick");
                    //ignore
                }
            }
        }

        return output.toString();
    }

    /**
     * Validates the answer returned by the shell
     *
     * @param answer
     * @return true if the answer contains the "success" word, false otherwise
     */
    private boolean validateAnswer(String answer) {
        log("validating answer: " + answer);
        if (answer == null || !answer.toLowerCase().contains("success")) {
            log("answer not valid: " + answer);
            return false;
        }

        return true;
    }

    private String buildCommand(String command, int device){
        StringBuilder sb = new StringBuilder(command);
        sb.append(device);
        sb.trimToSize();
        return sb.toString();
    }
    /**
     * Turns the device on
     *
     * @param device device id to turn on
     * @return true if the device is turned on, false otherwise
     */
    boolean on(int device) {
        return validateAnswer(
                executeCommand(buildCommand(COMMAND_ON, device)));
    }

    /**
     * Turns a device off
     *
     * @param device device id to turn off
     * @return true if the device was turned off, false otherwise
     */
    boolean off(int device) {
        return validateAnswer(
                executeCommand(buildCommand(COMMAND_OFF, device)));
    }

    /**
     * A period is an on part and an off part.
     *
     * @param on time in milliseconds the device must be on
     * @param off time in milliseconds the device must be off
     * @param devices device identifiers
     * @return true if the period was performed successfully
     * @throws InterruptedException in case of the thread is aborted during
     * sleep
     */
    boolean period(int on, int off, int[] devices) throws InterruptedException {
        for (int d : devices) {
            if (!on(d)) {
                log("on for " + d + " returned false");
                return false;
            }
        }
        Thread.sleep(on);

        for (int d : devices) {
            if (!off(d)) {
                log("off for " + d + " returned false");
                return false;
            }
        }
        Thread.sleep(off);
        return true;
    }

    /**
     * A period is an on part and an off part. This method performs multiple
     * periods.
     *
     * @param on time in milliseconds the device must be on in a period
     * @param off time in milliseconds the device must be off in a period
     * @param periods number of periods to perform
     * @param devices device identifiers
     * @return true if the periods was performed successfully
     * @throws InterruptedException in case of the thread is aborted during
     * sleep
     */
    boolean multiPeriod(int on, int off, int periods, int[] devices) throws InterruptedException {
        for (int i = 0; i < periods; i++) {
            if (!period(on, off, devices)) {
                log("period() returned failed");
                return false;
            }
        }
        return true;
    }

    /**
     * Sets the defined devices off
     *
     * @param devices to turn off
     */
    void resetDevices(int[] devices) {
        for (int d : devices) {
            off(d);
        }
    }
}
