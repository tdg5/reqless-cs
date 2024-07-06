.PHONY: qless-core
qless-core:
	make -C Reqless/qless-core/
	cp Reqless/qless-core/qless.lua Reqless/lua/

.PHONY: style
style:
	dotnet format whitespace
	dotnet format style
