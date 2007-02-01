CLEANFILES = SDCSServer.exe SDCSCommon.dll

SDCSServer_sources = AssemblyInfo.cs ConnectionWatcher.cs frmServer.cs ServerNetwork.cs UserDatabase.cs

resources = App.ico frmServer.resx

refs = System.Windows.Forms System.Data System.Drawing

embedded = $(foreach res,$(resources), $(addprefix -resource:,$(res)))

references = $(foreach ref,$(refs), $(addprefix -r:,$(ref)))
references += -r:SDCSCommon.dll

SDCSServer.exe: $(SDCSServer_sources) $(resources) SDCSCommon.dll
	$(MCS) $(references) $(SDCSServer_sources) $(embedded) -out:$@

SDCSCommon.dll:
	cd ../SDCSCommon && make && cp SDCSCommon.dll ../SDCSServer

all: SDCSServer.exe
