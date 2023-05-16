# Nothing Phone (1) LEDs
## Base path:
`/sys/devices/platform/soc/984000.i2c/i2c-0/0-0020/leds/aw210xx_led`

## Sets:
| Human friendly name | ID |
| --- | --- |
| Middle | `round_leds_br` |
| Camera | `rear_cam_led_br` |
| Diagonal | `front_cam_led_br` |
| Dot | `dot_led_br` |
| Line | `vline_leds_br` |

## Addressable:
Key: `/sys/devices/platform/soc/984000.i2c/i2c-0/0-0020/leds/aw210xx_led/single_led_br`
| Human friendly name | ID |
| --- | --- |
| Diagonal | `1` |
| Center bottom left | `2` |
| Center bottom right | `3` |
| Center top left | `5` |
| Center top right | `4` |
| Camera | `7` |
| Line (1-bottom) | `13` |
| Line (2) | `11` |
| Line (3) | `9` |
| Line (4) | `12` |
| Line (5) | `10` |
| Line (6) | `14` |
| Line (7) | `15` |
| Line (8-top) | `8` |
| Dot | `16` |
| Recording LED | `17` |

## Brightness range:
`0`-`4095`

## Set value:
`echo [BRIGHTNESS] > [LIGHT]`

## Get value:
`cat [LIGHT]`
