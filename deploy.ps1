# DevOps in a can!
remove-item -recurse -force ./build
neato monogame-release-build LD50 ./build
neato publish-itch ./build notexplosive ld50 continuous-jam