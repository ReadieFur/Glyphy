device=""
params=""

index=0
for arg in "$@"; do
    if [ $index -eq 0 ]; then
        device="$arg"
    elif [ -z "$params" ]; then
        params+="$arg"
    else
        params+=" $arg"
    fi
    ((index++))
done

echo $params > /sys/devices/platform/soc/984000.i2c/i2c-0/0-0020/leds/aw210xx_led/$device
