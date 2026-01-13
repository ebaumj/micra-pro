#!/bin/bash -e
if [ -f "${UPDATE_FILE}" ]; then
	if [ -d tmp ]; then
		rm -rf tmp
	fi
	mkdir tmp
	unzip "${UPDATE_FILE}" -d tmp
	chmod +x tmp/backend/MicraPro.Backend
	cp -rf tmp/* "${APPS_TARGET}/"
	rm "${UPDATE_FILE}"
	rm -rf tmp
	if [ -d "${CACHE_DIR}" ]; then
		rm -rf "${CACHE_DIR}"/*
	fi
fi
