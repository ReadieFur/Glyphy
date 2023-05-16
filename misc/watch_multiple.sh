files=()
modification_times=()

# Iterate over each command-line argument
for file in "$@"; do
    # Make sure the file exists
    if [ ! -f "$file" ]; then
        echo File \"$file\" does not exist.
        return
    fi

    files+=("$file")

    # Append each argument's last modification time to the array
    modification_times+=($(stat -c %Y "$file"))
done

while true; do
    # Sleep for a short duration
    sleep 0.01
    # sleep 1

    # Check each file that was passed in
    for index in "${!files[@]}"; do
        file="${files[$index]}"

        # Get the current and last known modification times
        current_modified=$(stat -c %Y "$file")
        last_modified="${modification_times[$index]}"

        # # Compare the current and last modification times
        if [[ $current_modified -ne $last_modified ]]; then
            echo File \"$file\" changed:
            # Do something when the file changes
            cat $file

            # Update the last modification time
            modification_times[$index]=$current_modified
        fi
    done
done
