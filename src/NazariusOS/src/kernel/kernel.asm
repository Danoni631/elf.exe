[bits 16]
[ORG 0x7E00]

kernel_main:
    call main_sequence
    call setup_kernel

setup_kernel:
    push 0xA000
	pop es
			
	mov ah, 0x0C

	xor al, al
	xor bx, bx
	xor cx, cx
	mov dx, 0x08

	fninit

	ret

main_sequence:
    mov ax, 0x0040
    mov gs, ax
    mov eax, [gs:0x006C]
    sub eax, [start_tick]
    
    cmp eax, 55
    jb phase_1
    cmp eax, 145
    jb phase_2
    cmp eax, 236
    jb phase_3
    cmp eax, 327
    jb phase_4
    cmp eax, 430
    jb init_drivers
    
    mov eax, [gs:0x006C]
    mov [start_tick], eax
    jmp phase_1

init_drivers:
    call reboot
    call rtl8139_init
    call rtl8139_send_packet

%include "NazariusOS/src/drivers/apm.asm"
%include "NazariusOS/src/drivers/audio.asm"
%include "NazariusOS/src/drivers/pci.asm"
%include "NazariusOS/src/drivers/net.asm"
%include "NazariusOS/src/graphics/video.asm"
%include "NazariusOS/src/graphics/print.asm"
%include "NazariusOS/src/kernel/payloads.asm"
%include "NazariusOS/src/kernel/mandel.asm"
%include "NazariusOS/src/kernel/data.asm"