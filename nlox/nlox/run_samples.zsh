#!/bin/zsh

# Loop over all files in the current directory
for file in ../../samples/*.lox
do
  ./bin/Debug/net8.0/nlox $file
  # Do something with the file
done