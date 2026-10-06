rtl8139_init:
    mov [pci_io_base], dx

    mov dx, [pci_io_base]
    add dx, REG_CONFIG1
    mov al, 0x00
    out dx, al

    mov dx, [pci_io_base]
    add dx, REG_CR
    mov al, 0x10            ; SFT-RST (Bit 4)
    out dx, al

.wait_reset:
    in al, dx
    test al, 0x10           ; Wait the board reset
    jnz .wait_reset

    mov dx, [pci_io_base]
    add dx, REG_RBSTART
    mov eax, rx_buffer
    out dx, eax

    mov dx, [pci_io_base]
    add dx, REG_IMR
    mov ax, 0x0005
    out dx, ax

    mov dx, [pci_io_base]
    add dx, REG_RCR
    mov eax, 0x0000000F     ; Broadcast, Multicast and other
    out dx, eax

    mov dx, [pci_io_base]
    add dx, REG_CR
    mov al, 0x0C
    out dx, al

    ret

rtl8139_send_packet:
    push edi
    
    mov edi, tx_buffer
    push cx
    movzx ecx, cx
    rep movsb
    pop cx

    mov dx, [pci_io_base]
    add dx, REG_TSAD0
    mov eax, tx_buffer
    out dx, eax

    mov dx, [pci_io_base]
    add dx, REG_TSD0
    movzx eax, cx 
    out dx, eax
    
    pop edi
    ret