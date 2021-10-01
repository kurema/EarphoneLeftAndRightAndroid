mkdir mipmap-mdpi
#4
convert -resize 48x48 inkscape.png mipmap-mdpi/icon.png
convert -resize 108x108 launcher.png mipmap-mdpi/launcher_foreground.png
mkdir mipmap-hdpi
#6
convert -resize 72x72 inkscape.png mipmap-hdpi/icon.png
convert -resize 162x162 launcher.png mipmap-hdpi/launcher_foreground.png
mkdir mipmap-xhdpi
#8
convert -resize 96x96 inkscape.png mipmap-xhdpi/icon.png
convert -resize 216x216 launcher.png mipmap-xhdpi/launcher_foreground.png
mkdir mipmap-xxhdpi
#12
convert -resize 144x144 inkscape.png mipmap-xxhdpi/icon.png
convert -resize 324x324 launcher.png mipmap-xxhdpi/launcher_foreground.png
mkdir mipmap-xxxhdpi
#16
convert -resize 192x192 inkscape.png mipmap-xxxhdpi/icon.png
convert -resize 432x432 launcher.png mipmap-xxxhdpi/launcher_foreground.png
