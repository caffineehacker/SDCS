CLEANFILES = SDCSClient.exe

SDCSClient_sources = AssemblyInfo.cs ClientNetwork.cs frmClient.cs frmConnectToServer.cs MainList.cs

resources = App.ico frmClient.resx frmConnectToServer.resx MainList.resx

refs = System.Windows.Forms System.Data System.Drawing

embedded = $(foreach res,$(resources), $(addprefix -resource:,$(res)))

references = $(foreach ref,$(refs), $(addprefix -r:,$(ref)))
references += -r:../SDCSCommon/SDCSCommon.dll

SDCSClient.exe: $(SDCSClient_sources) $(resources)
	$(MCS) $(references) $(SDCSClient_sources) $(embedded) -out:$@

all: SDCSClient.exe
