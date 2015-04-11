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
        self._password_hash = None
        self._public_key_path = None
        self._private_key_path = None

    @property
    def public_key_path(self):
        return self._public_key_path

    @public_key_path.setter
    def public_key_path(self, value):
        self._public_key_path = value

    @property
    def private_key_path(self):
        return self._private_key_path

    @private_key_path.setter
    def private_key_path(self, value):
        self._private_key_path = value

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
    def password_hash(self):
        return self._password_hash

    @password_hash.setter
    def password_hash(self, value):
        self._password_hash = value
