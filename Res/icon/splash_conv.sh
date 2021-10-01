mkdir drawable-mdpi
#4
convert -resize 480x480 splash.png drawable-mdpi/splash_logo.png
convert -resize 320x splash_title.png drawable-mdpi/splash_title.png
mkdir drawable-hdpi
#6
convert -resize 720x720 splash.png drawable-hdpi/splash_logo.png
convert -resize 480x splash_title.png drawable-hdpi/splash_title.png
mkdir drawable-xhdpi
#8
cp -f splash.png drawable-xhdpi/splash_logo.png
convert -resize 640x splash_title.png drawable-xhdpi/splash_title.png
#convert -resize 240x240 splash.png drawable-xhdpi/splash_logo.png
mkdir drawable-xxhdpi
#12
#convert -resize 360x360 splash.png drawable-xxhdpi/splash_logo.png
cp -f splash_title.png drawable-xxhdpi/splash_title.png
#mkdir drawable-xxxhdpi
#16
#convert -resize 480x480 splash.png drawable-xxxhdpi/splash_logo.png
