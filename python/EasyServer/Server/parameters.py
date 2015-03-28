# EasyServer to listen and send answer to incoming socket call
# Bela Bursan<burszan@gmail.com> - 2015-02-28
#


class Parameters(object):
    """
    Property class for the server
    Holds settings, properties
    """

    def __init__(self):
        """
        Sets the default values needed for the server
        :return: void
        """
        self._port = 2015
        self._debug = False
        self._timeout_seconds = 16
        self._noof_sockets = 10
        self._noof_threads = 10
        self._crypt_key = "A2B3C4D5E6F7"

    @property
    def port(self):
        """
        Returns the port the server listens for incoming calls
        :return: port as integer
        """
        return self._port

    @port.setter
    def port(self, value):
        """
        Setst the listen port for the server
        :param value: port as int
        :return: void
        """
        self._port = value

    @property
    def debug(self):
        return self._debug

    @debug.setter
    def debug(self, value):
        self._debug = value

    @property
    def timeout_seconds(self):
        return self._timeout_seconds

    @timeout_seconds.setter
    def timeout_seconds(self, value):
        self._timeout_seconds = value

    @property
    def noof_sockets(self):
        return self._noof_sockets

    @noof_sockets.setter
    def noof_sockets(self, value):
        self._noof_sockets = value

    @property
    def noof_threads(self):
        return self._noof_threads

    @noof_threads.setter
    def noof_threads(self, value):
        self._noof_threads = value

    @property
    def crypt_key(self):
        return self._crypt_key

    @crypt_key.setter
    def crypt_key(self, value):
        self._crypt_key = value
