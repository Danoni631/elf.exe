[bits 16]
[ORG 0x7C00]

%define KERNEL 0x7E00
%define SECTORS 0x0005
%define VIDEOMODE 0x0013

xor ax, ax
mov ds, ax
mov es, ax

mov sp, 0x7C00
mov ss, ax

call set_video_mode
call kernel_sectors

jmp 0x0000:KERNEL

kernel_sectors:
    mov ah, 0x02
    mov al, SECTORS
    mov ch, 0x00
    mov cl, 0x02
    mov dh, 0x00

    xor bx, bx
    mov es, bx

    mov bx, KERNEL

    int 0x13

    ret

set_video_mode:
    mov ax, VIDEOMODE
    int 0x10

    ret

times 510 - ($ - $$) db 0x00
dw 0xAA55