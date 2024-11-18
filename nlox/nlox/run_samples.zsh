#!/bin/zsh

# Loop over all files in the current directory
for file in /Users/croese/code/crafting-interpreters/samples/*.lox
do
  /Users/croese/code/crafting-interpreters/NLox/NLox/bin/Debug/net8.0/nlox $file
  # Do something with the file
done