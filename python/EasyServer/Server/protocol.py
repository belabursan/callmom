# EasyServer to listen and send answer to incoming socket call
# Bela Bursan<burszan@gmail.com> - 2015-03-27
#
import logging
import time
import sys

# send commands
EXIT = "exit\n" # .encode()
WELCOME = "welcome\n"
DONE = "done\n"

# receive commands
HELLO = "hello"
FLASH_LAMP = "flash_lamp"
COMMAND = "command"

BUFFER_SIZE = 256


class Protocol(object):
    """
    Protocol implementation
    """
    def __init__(self, connection, parameter):
        """
        Constructor
        :param connection: connected socket
        :param parameter: parameter object, holds the parameters
        """
        self._connection = connection
        self._parameter = parameter

    def execute(self):
        """
        Entry point for the protocol
        :return:
        """
        logging.debug("Protocol:run(): running the protocol")
        if not self.handshake():
            logging.info("Protocol:run(): handshake not successful")
            return
        logging.debug("Protocol:run(): handshake successful")
        self.wait_for_command()

        time.sleep(5)
        return

    def handshake(self):
        """
        Waits for a Handshake or times out with exception
        :return: True if the handshake is ok, False otherwise
        :exception: throws Socket.timeout exception
        """
        data = self._connection.recv(BUFFER_SIZE).strip()
        if HELLO == data:
            self._connection.sendall(WELCOME)
            return True
        else:
            self._connection.sendall(EXIT)
        return False

    def wait_for_command(self):
        """
        Waits for a command or times out with exception
        :return: True if the commands was received and executed successfully, False otherwise
        :exception: throws Socket.timeout exception
        """
        data = self._connection.recv(BUFFER_SIZE).strip()
        try:
            if data.startswith(COMMAND):
                command = data.split(":")
                # c = str(command[1])
                if FLASH_LAMP == command[1]:
                    if self.do_flash_lamp(command[2], command[3], command[4]):
                        return True
                else:
                    # handle other commands
                    pass
            else:
                # handle other messages
                pass
        except:
            logging.info("Protocol:wait_for_command(): error when executing command: " + sys.exc_info()[0])
            pass

        self._connection.sendall(EXIT)
        return False

    def do_flash_lamp(self, flash_time, do_blink, interval_seconds):
        """
        Executes the flash lamp command
        Calls the Tellstick Duo and makes a lamp flash
        :param flash_time: int, seconds time to flash
        :param do_blink: bool, true/false
        :param interval_seconds: int, if blink then interval time
        :return: true if command executed successfully, false otherwise
        """
        # todo - add tellstick code here
        logging.debug("Protocol:do_flash_lamp("+flash_time+", " + do_blink + ", " + interval_seconds + "): start")
        self._connection.sendall(DONE)
        return True
