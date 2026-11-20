USE retrogame;

SET @idUsuario = 1;
SET @idPedido = 1;
SET @idJogo = 1;

SELECT co_situacao_pedido, ds_situacao_pedido FROM tb_situacao_pedido ORDER BY co_situacao_pedido;

SELECT co_seq_pedido, dt_pedido, vl_total, co_situacao_pedido
FROM tb_pedido WHERE co_usuario = @idUsuario ORDER BY dt_pedido DESC, co_seq_pedido DESC;

SELECT co_jogo, qt_item, vl_unitario
FROM tb_item_pedido WHERE co_pedido = @idPedido ORDER BY co_seq_item_pedido;

SELECT co_seq_jogo, no_jogo, qt_estoque FROM tb_jogo WHERE co_seq_jogo = @idJogo;
