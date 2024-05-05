#vscode sucks so much

import shutil
import sys
import os

def copy_folder(source_folder, destination_folder):
    shutil.copytree(source_folder, destination_folder, dirs_exist_ok= True)

copy_folder(sys.argv[1], sys.argv[2])
