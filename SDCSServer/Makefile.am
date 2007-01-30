CLEANFILES = SDCSServer.exe

SDCSServer_sources = AssemblyInfo.cs ConnectionWatcher.cs frmServer.cs ServerNetwork.cs

resources = App.ico frmServer.resx

refs = System.Windows.Forms System.Data System.Drawing

embedded = $(foreach res,$(resources), $(addprefix -resource:,$(res)))

references = $(foreach ref,$(refs), $(addprefix -r:,$(ref)))
references += -r:../SDCSCommon/SDCSCommon.dll

SDCSServer.exe: $(SDCSServer_sources) $(resources)
	$(MCS) $(references) $(SDCSServer_sources) $(embedded) -out:$@

all: SDCSServer.exe
