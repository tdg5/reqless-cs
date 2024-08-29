.PHONY: reqless-core
reqless-core:
	make -C Reqless/Client/reqless-core/
	cp Reqless/Client/reqless-core/reqless.lua Reqless/Client/LuaScripts/

.PHONY: style
style:
	dotnet format whitespace
	dotnet format style
