import GMTool.settings

def global_info(request):
	globalRunMode = str(GMTool.settings.g_runMode)

	return locals()



