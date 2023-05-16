# Nothing Phone (1) LEDs
## Base path:
`/sys/devices/platform/soc/984000.i2c/i2c-0/0-0020/leds/aw210xx_led`

## Sets:
| Human friendly name | ID |
| --- | --- |
| Battery | `round_leds_br` |
| Camera | `rear_cam_led_br` |
| Diagonal | `front_cam_led_br` |
| USB dot | `dot_led_br` |
| USB line | `vline_leds_br` |

## Addressable:
Key: `/sys/devices/platform/soc/984000.i2c/i2c-0/0-0020/leds/aw210xx_led/single_led_br`
| Human friendly name | ID |
| --- | --- |
| Diagonal | `1` |
| Bottom left | `2` |
| Bottom right | `3` |
| Top left | `5` |
| Top right | `4` |
| Camera | `7` |
| USB line (1-bottom) | `13` |
| USB line (2) | `11` |
| USB line (3) | `9` |
| USB line (4) | `12` |
| USB line (5) | `10` |
| USB line (6) | `14` |
| USB line (7) | `15` |
| USB line (8-top) | `8` |
| USB dot | `16` |
| Camera RED | `17` |

## Brightness range:
`0`-`4095`

## Set value:
`echo [BRIGHTNESS] > [LIGHT]`

## Get value:
`cat [LIGHT]`
