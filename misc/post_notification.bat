@REM https://android.stackexchange.com/questions/221089/send-a-notification-or-just-vibrate-to-phone-via-adb-shell
adb shell cmd notification post -S bigtext -t 'Title' 'Tag' 'Multiline text'
