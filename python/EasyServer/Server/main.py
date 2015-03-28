#!/usr/bin/python
#
# EasyServer to listen and send answer to incoming socket call
# Bela Bursan<burszan@gmail.com> - 2015-02-28
#
import sys
import os
import traceback
import atexit
import logging
import time
import socket
import getpass
from parameters import Parameters
from server import Server
try:
    import configparser
except:
    from six.moves import configparser

__author__ = 'buri'
DEFAULT_APP_NAME = "EasyServer"


class Main(object):
    """
    Main is the entry point for the server, reads parameters and runs the server
    """
    def __init__(self, parameters):
        """
        Constructor
        :param parameters: parameters from file or command line
        """
        atexit.register(self.shut_down_hook, None, None)
        self._parameters = parameters
        self._server = Server(self)

    def shut_down_hook(self, targs, kargs):
        """ Shut dow hook, called by the system if an ending signal is received
        :param targs: not used
        :param kargs: not used
        """
        logging.warning("Main:shut_down_hook(): GOT EXIT SIGNAL in shut down hook")
        self._server.stop_server()

    def run(self):
        """
        The Main function
        :rtype : int - 0 if success
        """
        try:
            self.server.start()
            self.server.join()
        except KeyboardInterrupt:
            logging.warning("Main:run(): Got KeyboardInterrupt")

        except socket.error as ex:
            logging.warning("Main:run(): Got socket exception : " + str(ex))
            pass
        finally:
            self.server.stop_server()
        return 0

    @property
    def parameters(self):
        return self._parameters

    @property
    def server(self):
        return self._server

# end of class


def setup_logging(arguments, force_debug):
    """
    Initiates the logger
    :param arguments: arguments for the logger
    :param force_debug: boolean, true if debug mode shall be forced
    """
    loglevel = logging.INFO
    logformat = '[%(asctime)s] %(levelname)s:(%(processName)-2s) - %(message)s'
    datetimeformat = '%Y-%m-%d %H:%M:%S'
    logfilemode = 'w'
    logfilename = None

    if len(arguments) > 0:
        if force_debug is True or any(str(x).upper() in "DEBUG" for x in arguments):
            loglevel = logging.DEBUG
        elif any(str(x).upper() in "RELEASE" for x in arguments):
            loglevel = logging.WARNING

        if any(str(x).upper() in "LOGTOFILE" for x in arguments):
            logfilename='easy_server.log'

            if any(str(x).upper() in "APPEND" for x in arguments):
                logfilemode='a'
    elif force_debug is True:
        loglevel = logging.DEBUG

    logging.basicConfig(level=loglevel, format=logformat, datefmt=datetimeformat, filename=logfilename, filemode=logfilemode)
    logging.warning("Started ("+str(logging.getLevelName(loglevel)) +
                    ")\n---------------------------------------------------------------------------------")

def read_parameters():
    """
    Reads parameters from a file located in the current running directory
    The file name must be "easy_config.ini"
    :returns: the parameters read from the file or default parameters as Parameters object
    """
    # print("reading parameters from configuration file")
    _parameters = Parameters()
    parser = configparser.RawConfigParser(allow_no_value=True)
    try:
        config_ini = open("easy_config.ini", "r+")
        parser.readfp(config_ini)

        _parameters.password_hash = parser.get("ENCRYPTION", "password_hash")
        if not _parameters.password_hash:
            from crypto import BCrypt
            crypt = BCrypt(None, None, 0)
            password = getpass.getpass("\nA password not seems to be set or is wrong!\n"
                                       "Please set a new hash by typing a password:\n")
            password_hash = crypt.generate_hash(password)
            parser.set("ENCRYPTION", "password_hash", password_hash)
            parser.write(open("easy_config.ini", "w"))

            _parameters.password_hash = parser.get("ENCRYPTION", "password_hash")
            if not _parameters.password_hash:
                print("Could not read password hash, ending server!")
                return None

        _parameters.port = parser.getint("NETWORK", "port")
        _parameters.noof_sockets = parser.getint("NETWORK", "noof_sockets")
        _parameters.timeout_seconds = parser.getint("NETWORK", "timeout")
        _parameters.debug = parser.getboolean("DEBUG", "debug")
        _parameters.noof_threads = parser.getint("OTHER", "noof_threads")

    except Exception as excp:
        print("Exception when reading parameters:" + str(excp))
        return None
    return _parameters


if __name__ == '__main__':
    """
    Star point, calls the main function
    Catches every exception
    """
    try:
        print("Hello, now running " + DEFAULT_APP_NAME)
        print("    Available command line arguments are:")
        print("      - debug     : sets loglevel to DEBUG")
        print("      - release   : sets loglevel to WARNING")
        print("      - logtofile : logs are saved in a logfile")
        print("      - append    : logs are appended to existing logfile")
        print("")
        print("    Return values are:")
        print("      - 0 : interrupted by user")
        print("      - 1 : in case of error")
        print("")

        time.sleep(0.2)
        params = read_parameters()
        if params:
            setup_logging(sys.argv[1:], params.debug)
            main = Main(params)
            os._exit(main.run())
        else:
            os._exit(1)
    except BaseException as ex:  # catch everything
        logging.exception("\nMain:main(): [ERROR ] - Unhandled Exception - [ERROR ]:\n")
        traceback.print_exc(file=sys.stdout)
        sys.exit(1)

