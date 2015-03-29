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
VERSION = "v0.1"
SPLITTER = ":"
LINE_END = "\n"


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

    def do_split(self, data):
        """
        Splits a command with the SPLITTER and returns an array with the parts
        :param data: data to split
        :return: array of strings
        :except: if there is no SPLITTER
        """
        if SPLITTER in data:
            return data.split(SPLITTER)
        raise ValueError("Data is unsplittable")

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
        self.handle_messages()

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
            self.write(WELCOME + SPLITTER + VERSION + LINE_END)
            return True
        else:
            self.write(EXIT)
        return False

    def handle_messages(self):
        """
        Waits for a message or times out with exception
        :return: True if the messages was received and executed successfully, False otherwise
        :exception: throws Socket.timeout exception
        """
        data = self.read()
        success = False
        try:
            message = self.do_split(data)
            if message[0] == COMMAND:
                success = self.do_handle_commands(message[1])
            elif message[0] == REGISTER:
                success = self.do_register(message[1])
                pass
            else:
                # handle other messages
                pass
        except Exception as ex:
            logging.warning("Protocol:handle_messages(): got "
                            + str(sys.exc_info()[0])
                            + " when executing message: "
                            + ex.message)

        self.write(EXIT)
        return success

    def do_register(self, crypto_command):
        """
        Registers an app, if password verification is ok the public key is returned as answer
        :param crypto_command: takes a parameter with two parts separated with colon:
        first is the hash of the password the server needs to compare with its own hash,
        second part is a random number
        :return: true if registered successfully, false otherwise
        """
        success = False
        logging.info("Protocol:do_register(): registering new user")

        try:
            command_line = self._crypter.decrypt_AES(self._parameter.password_hash, crypto_command)
            command = self.do_split(command_line)
            if command[0] == self._parameter.password_hash:
                # password is ok, send the public key
                public_key = open(self._parameter.public_key_path, "r")
                self.write(public_key.read(BUFFER_SIZE))
                success = True
        except Exception as ex:
            logging.warning("Protocol:do_register(): got "
                            + str(sys.exc_info()[0])
                            + " when executing registering: "
                            + str(ex.message))
        self.write(EXIT)
        return success

    def do_handle_commands(self, crypto_command):
        """
        Handles commands
        :param crypto_command: encrypted command line that should be decrypted with the private key
        :return:
        """
        logging.info("Protocol:do_handle_commands(): new command")
        success = False

        try:
            rsa_key = self._crypter.create_RSA_key(self._parameter.private_key_path)
            command_line = self._crypter.decrypt_RSA(rsa_key, crypto_command)
            command = self.do_split(command_line)
            if FLASH_LAMP == command[0]:
                success = self.do_flash_lamp(command[1], command[2], command[3])
            # add more commands here
            else:
                logging.warning("Protocol:do_handle_commands() received unrecognized command: " + str(command[0]))
        except Exception as ex:
            logging.warning("Protocol:do_handle_commands() exception when handling commands: " + str(ex.message))
        return success

    def do_flash_lamp(self, flash_time, do_blink, interval_seconds):
        """
        Executes the flash lamp command
        Calls the Tellstick Duo and makes a lamp flash
        :param flash_time: int, seconds time to flash
        :param do_blink: bool, true/false
        :param interval_seconds: int, if blink then interval time
        :return: true if command executed successfully, false otherwise
        """
        # todo - add tell-stick code here
        logging.debug("Protocol:do_flash_lamp("+flash_time+", " + do_blink + ", " + interval_seconds + "): start")
        self.write(DONE)
        return True
