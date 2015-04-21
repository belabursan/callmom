# EasyServer to listen and send answer to incoming socket call
# Bela Bursan<burszan@gmail.com> - 2015-03-27
#
import logging
import base64
import time
import sys
from crypto import BCrypt

# send commands
EXIT = "Exit"
WELCOME = "Welcome"
SUCCESS = "Success"
FAILED = "Failed"

# receive commands
HELLO = "H"
FLASH_LAMP = "Flash_lamp"
COMMAND = "Command"
REGISTER = "Register"
XCHANGEKEY = "X"

BUFFER_SIZE = 8192
VERSION = "v0.0.1"
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
        self._crypter = BCrypt(parameter)
        self._key = None

    def read(self):
        return self._connection.recv(BUFFER_SIZE).decode('utf-8').strip()

    def write(self, data):
        d = (data + LINE_END).encode('utf-8')
        self._connection.sendall(d)

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

        self.handshake()
        self.handle_messages()
        if self._key is None:
            # it probably was a register command
            logging.info("Protocol:run(): registered new user successfully")
            return
        self.handle_messages()
        self.write(EXIT)
        time.sleep(5)
        return

    def handshake(self):
        """
        Waits for a Handshake or times out with exception
        :return: True if the handshake is ok, False otherwise
        :exception: throws Socket.timeout exception
        """
        data = self.read()
        if HELLO != data:
            logging.warning("Protocol:handshake(): handshake failed")
            raise ValueError("Handshake failed(" + data + ")")

        self.write(WELCOME + SPLITTER + VERSION)
        logging.info("Protocol:handshake(): handshake successful")

    def get_key(self):
        logging.debug("Protocol:get_key(): start")
        data = self.read()
        data = self.do_split(data)
        if data[0] is not XCHANGEKEY:
            raise ValueError("Expecting command " + XCHANGEKEY)
        self._key = self._crypter.decrypt_RSA(self._parameter.private_key_path, data[1])
        self.write(SUCCESS)



    def handle_messages(self):
        """
        Waits for a message or times out with exception
        :return: True if the messages was received and executed successfully, False otherwise
        :exception: throws Socket.timeout exception
        """
        logging.debug("Protocol:handle_messages(): new message")

        data = self.read()

        message = self.do_split(data)

        if message[0] == COMMAND:
            self.do_handle_commands(message[1])
        elif message[0] == REGISTER:
            self.do_register(message[1])
        elif message[0] == XCHANGEKEY:
            self.get_key()
        # handle other messages
        else:
            logging.warning("Protocol:handle_messages(): got " + message[0])
            raise ValueError("Unrecognized message: " + message[0])

    def do_register(self, crypto_command):
        """
        Registers an app, if password verification is ok the public key is returned as answer
        :param crypto_command: takes a parameter with two parts separated with colon:
        first is the hash of the password the server needs to compare with its own hash,
        second part is a random number
        :return: true if registered successfully, false otherwise
        """
        logging.debug("Protocol:do_register(): registering new user")

        try:
            command_line = self._crypter.decrypt_AES(self._parameter.password_hash, crypto_command)
            command = self.do_split(command_line)
            if command[0] == self._parameter.password_hash:
                # password is ok, send the public key
                public_key = open(self._parameter.public_key_path, "r").read(BUFFER_SIZE)
                random = self._crypter.create_random(64)
                crypto = self._crypter.encrypt_AES(self._parameter.password_hash, public_key + SPLITTER + random)

                self.write(REGISTER + SPLITTER + crypto)

        except Exception as ex:
            logging.warning("Protocol:do_register(): got "
                            + str(sys.exc_info()[0])
                            + " when executing registering: "
                            + str(ex.message))

    def do_handle_commands(self, crypto_command, key):
        """
        Handles commands
        :param crypto_command: encrypted command line that should be decrypted with the private key.
        The command is built up in several chunks separated with colon,
        the last chunk is always a random sequence of characters
        :return: true if the execution of the command was successful, false otherwise
        """
        logging.info("Protocol:do_handle_commands(): new command")
        success = False

        try:
            # rsa_key = self._crypter.create_RSA_key(self._parameter.private_key_path)
            command_line = self._crypter.decrypt_RSA(self._parameter.private_key_path, crypto_command)
            # cc = str(command_line)
            # vv = command_line.decode()
            # bb = command_line.decode("utf-8")
            command = self.do_split(command_line)
            if FLASH_LAMP == command[0]:
                success = self.do_flash_lamp(command[1], command[2], command[3])
            # todo - add more commands here
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
