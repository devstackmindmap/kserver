#!/bin/bash

kill -15 $(lsof -t -i:30654)
