#!/usr/bin/env python
"""
Command-line utility for administrative tasks.
"""

import os, sys

if __name__ == "__main__":
	os.environ.setdefault(
		"DJANGO_SETTINGS_MODULE",


		"GMTool.settings"
	)

	from django.core.management import execute_from_command_line

	from GMTool.settings import g_runMode, g_args
	print('runMode is ' + g_runMode)

	execute_from_command_line(g_args)

