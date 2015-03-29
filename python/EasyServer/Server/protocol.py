# EasyServer to listen and send answer to incoming socket call
# Bela Bursan<burszan@gmail.com> - 2015-03-27
#
import logging
import time
import sys
from crypto import BCrypt

# send commands
EXIT = "exit\n"
WELCOME = "welcome\n"
DONE = "done\n"

# receive commands
HELLO = "hello"
FLASH_LAMP = "flash_lamp"
COMMAND = "command"
REGISTER = "register"

BUFFER_SIZE = 512


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
        self._crypter = BCrypt(parameter, padding='{', block_size=16)

    def read(self):
        return self._connection.recv(BUFFER_SIZE).decode('utf8').strip()

    def write(self, data):
        self._connection.sendall(data.encode('utf8'))

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
        self.handle_commands()

        time.sleep(5)
        return

    def handshake(self):
        """
        Waits for a Handshake or times out with exception
        :return: True if the handshake is ok, False otherwise
        :exception: throws Socket.timeout exception
        """
        data = self.read()
        if HELLO == data:
            self.write(WELCOME)
            return True
        else:
            self.write(EXIT)
        return False

    def handle_commands(self):
        """
        Waits for a command or times out with exception
        :return: True if the commands was received and executed successfully, False otherwise
        :exception: throws Socket.timeout exception
        """
        data = self.read()
        try:
            command = data.split(":")
            if command[0] == COMMAND:
                if FLASH_LAMP == command[1]:
                    if self.do_flash_lamp(command[2], command[3], command[4]):
                        return True
                else:
                    # handle other commands
                    pass
            elif command[0] == REGISTER:
                self.do_register(command[1])
                pass
            else:
                # handle other messages
                pass
        except:
            logging.info("Protocol:wait_for_command(): error when executing command: " + sys.exc_info()[0])
            pass

        self.write(EXIT)
        return False

    def do_register(self, register):
        """
        Registers an app, if password verification is ok the public key is returned as answer
        :param register: takes a parameterwith two parts separated with colon:
        first is the hash of the password the server needs to compare with its own hash,
        second part is a random number
        :return:
        """
        data = self._crypter.decrypt_AES(self._parameter.password_hash, register)
        data_list = data.split(":")
        if data_list[0] == self._parameter.password_hash:
            # password is ok, send the public key
            public_key = open(self._parameter.publik_key_path, "r")
            self.write(public_key.read(BUFFER_SIZE))
        pass

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
        self.write(DONE)
        return True
