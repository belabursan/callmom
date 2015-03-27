# EasyServer to listen and send answer to incoming socket call
# Bela Bursan<burszan@gmail.com> - 2015-03-25
#
import logging
import time
import StringIO


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
            return

        time.sleep(5)

    def handshake(self):
        """
        Handshake
        :return:
        """
        EXIT = "exit"
        HELLO = "hello"
        WELCOME = ['w','e','l','c','o','m','e']
        FLASH_LAMP = "flash_lamp"
        HELLO2 = ['h', 'e', 'l', 'l', 'o', '\r', '\n']

# data = input("SEND(Type q or Q to quit):")

        rdata = self._connection.recv(1024)
        logging.debug("Protocol:handshake(): Received: " + str(rdata))
        handshaked = False

        if HELLO2 != rdata:
            self._connection.sendall(StringIO.StringIO(WELCOME))
            handshaked = True
        else:
            self._connection.send(EXIT)
        return handshaked
