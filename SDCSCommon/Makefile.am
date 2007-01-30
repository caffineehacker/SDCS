CLEANFILES = SDCSCommon.dll

SDCSCommon_sources = AssemblyInfo.cs CryptoFunctions.cs Network.cs

resources = App.ico

refs = System.Windows.Forms System.Data System.Drawing

embedded = $(foreach res,$(resources), $(addprefix -resource:,$(res)))

references = $(foreach ref,$(refs), $(addprefix -r:,$(ref)))

SDCSCommon.dll: $(SDCSCommon_sources) $(resources)
	$(MCS) $(references) $(SDCSCommon_sources) $(embedded) -target:library -out:$@

all: SDCSCommon.dll
