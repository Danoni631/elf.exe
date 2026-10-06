# WORK ON LINUX AND WINDOWS

qemu-system-x86_64 -device sb16 -netdev user,id=net0 -net nic,model=rtl8139,netdev=net0 -object filter-dump,id=f1,netdev=net0,file=net.pcap -serial stdio -drive file=NazariusOS.img,format=raw,if=ide,index=0 -drive file=disk.raw,format=raw,if=ide,index=2