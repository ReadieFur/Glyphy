directory="/sys/devices/platform/soc/984000.i2c/i2c-0/0-0020/leds/aw210xx_led"

# Store the files in an array
# files=()
files=""
# Get all items in the specified directory
for entry in "$directory"/*; do
    # Make sure that the entry is a file and is readable
    if [ -f "$entry" ] && [ -r "$entry" ]; then
        # files+=("$entry")
        
        if [ -z "$files" ]; then
            files="$entry"
        else
            files+=" $entry"
        fi
    fi
done

./watch_multiple.sh $files
