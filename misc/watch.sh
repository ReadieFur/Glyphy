file="$1"

# Get initial modification time
last_modified=$(stat -c %Y "$file")

while true; do
    # Sleep for a short duration
    sleep 0.01

    # Get the current modification time
    current_modified=$(stat -c %Y "$file")

    # Compare the current and last modification times
    if [[ $current_modified -ne $last_modified ]]; then
        # echo "File changed!"
        # Do something when the file changes
        cat $file

        # Update the last modification time
        last_modified=$current_modified
    fi
done
