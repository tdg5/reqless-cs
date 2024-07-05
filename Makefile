.PHONY: reqless-core
reqless-core:
	make -C Reqless/reqless-core/
	cp Reqless/reqless-core/reqless.lua Reqless/lua/

.PHONY: style
style:
	dotnet format whitespace
	dotnet format style
