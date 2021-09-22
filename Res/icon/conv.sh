mkdir mipmap-hdpi
convert -resize 72x72 inkscape.png mipmap-hdpi/icon.png
convert -resize 162x162 launcher.png mipmap-hdpi/launcher_foreground.png
mkdir mipmap-mdpi
convert -resize 48x48 inkscape.png mipmap-mdpi/icon.png
convert -resize 108x108 launcher.png mipmap-mdpi/launcher_foreground.png
mkdir mipmap-xhdpi
convert -resize 96x96 inkscape.png mipmap-xhdpi/icon.png
convert -resize 216x216 launcher.png mipmap-xhdpi/launcher_foreground.png
mkdir mipmap-xxhdpi
convert -resize 144x144 inkscape.png mipmap-xxhdpi/icon.png
convert -resize 324x324 launcher.png mipmap-xxhdpi/launcher_foreground.png
mkdir mipmap-xxxhdpi
convert -resize 192x192 inkscape.png mipmap-xxxhdpi/icon.png
convert -resize 432x432 launcher.png mipmap-xxxhdpi/launcher_foreground.png
