'''
pre-command
'''
import sys
import shutil
import os.path
from performance.logger import setup_loggers
from shared import const
from shared.precommands import PreCommands
from test import EXENAME

setup_loggers(True)
precommands = PreCommands()
precommands.new(template='mvc',
                output_dir=const.APPDIR,
                bin_dir=const.BINDIR,
                exename=EXENAME,
                working_directory=sys.path[0])
precommands.execute()
shutil.copy(os.path.join('src', 'Program.cs'), os.path.join(const.APPDIR, 'Program.cs'))

# Update TFM to net7.0 while we wait for the default to be updated
f = open(os.path.join(os.getcwd(), "app", "mvcinnerloop.csproj"), 'r')
outFileText = ""
for line in f.readlines():
    line = line.replace('net6.0', 'net7.0')
    outFileText += line
f.close()
os.remove(os.path.join(os.getcwd(), "app", "mvcinnerloop.csproj"))
f = open(os.path.join(os.getcwd(), "app", "mvcinnerloop.csproj"), 'w')
f.write(outFileText)
f.close()