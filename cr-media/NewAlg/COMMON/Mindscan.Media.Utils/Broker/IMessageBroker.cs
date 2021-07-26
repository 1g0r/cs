using System;
using System.Collections.Generic;
using Mindscan.Media.Utils.Broker.Consumer;

namespace Mindscan.Media.Utils.Broker
{
	public interface IMessageBroker : IDisposable
	{
		void Send<T>(string exchange, T message);
		void Send<T>(string exchange, T message, Dictionary<string, string> headers);
		void Send<T>(string exchange, string routingKey, T message);
		void Send<T>(string exchange, string routingKey, T message, Dictionary<string, string> headers);
		void Publish<T>(T message);

		/// <summary>
		/// Подписывает обработчик сообщений на очередь. Обработчик будет принимать все сообщения не зависимо от типа.
		/// </summary>
		/// <typeparam name="TConsumer">Тип обработчика.</typeparam>
		/// <param name="queue">Название очереди, на которую нужно подписаться. Создает очередь, если её не было.</param>
		/// <param name="prefetchCount">Количество одновременно обрабатываемых сообщений, если 0, то значение берется из настройки.</param>
		/// <param name="consumersCount">Количество параллельно подключаемых обработчиков.</param>
		/// <returns>Брокер сообщений <see cref="IMessageBroker"/>.</returns>
		IMessageBroker Subscribe<TConsumer>(
			string queue, 
			ushort prefetchCount = 0, 
			ushort consumersCount = 1) where TConsumer : MediaConsumerBase;

		/// <summary>
		/// Подписывает обработчик сообщений на очередь. Обработчик будет принимать все сообщения не зависимо от типа.
		/// </summary>
		/// <typeparam name="TConsumer">Тип обработчика.</typeparam>
		/// <param name="queue">Название очереди, на которую нужно подписаться. Создает очередь, если её не было.</param>
		/// <param name="exchange">Название обменника, в который будут отсылаться сообщения. Создает обменник, если его не было.</param>
		/// <param name="prefetchCount">Количество одновременно обрабатываемых сообщений, если 0, то значение берется из настройки.</param>
		/// <param name="consumersCount">Количество параллельно подключаемых обработчиков.</param>
		/// <returns>Брокер сообщений <see cref="IMessageBroker"/>.</returns>
		IMessageBroker Subscribe<TConsumer>(
			string queue, 
			string exchange, 
			ushort prefetchCount = 0, 
			ushort consumersCount = 1) where TConsumer : MediaConsumerBase;

		/// <summary>
		/// Подписывает обработчик сообщений на очередь. Обработчик будет принимать все сообщения не зависимо от типа.
		/// </summary>
		/// <typeparam name="TConsumer">Тип обработчика.</typeparam>
		/// <param name="queue">Название очереди, на которую нужно подписаться. Создает очередь, если её не было.</param>
		/// <param name="exchange">Название обменника, в который будут отсылаться сообщения. Создает обменник, если его не было.</param>
		/// <param name="routingKey">Ключ маршрутизации. Привязывает очередь к обменнику с помощью этого ключа. Если не пустой, то обменник создается с типом direct.</param>
		/// <param name="prefetchCount">Количество одновременно обрабатываемых сообщений, если 0, то значение берется из настройки.</param>
		/// <param name="consumersCount">Количество параллельно подключаемых обработчиков.</param>
		/// <returns>Брокер сообщений <see cref="IMessageBroker"/>.</returns>
		IMessageBroker Subscribe<TConsumer>(
			string queue, 
			string exchange, 
			string routingKey, 
			ushort prefetchCount = 0, 
			ushort consumersCount = 1) where TConsumer : MediaConsumerBase;

		/// <summary>
		/// Подписывает обработчик сообщений на очередь. Обработчик будет принимать только сообщения указанного типа.
		/// </summary>
		/// <typeparam name="TMessage">Тип сообщения.</typeparam>
		/// <typeparam name="TConsumer">Тип обработчика.</typeparam>
		/// <param name="queue">Название очереди, на которую нужно подписаться. Создает очередь, если её не было.</param>
		/// <param name="prefetchCount">Количество одновременно обрабатываемых сообщений, если 0, то значение берется из настройки.</param>
		/// <param name="consumersCount">Количество параллельно подключаемых обработчиков.</param>
		/// <returns>Брокер сообщений <see cref="IMessageBroker"/>.</returns>
		IMessageBroker Subscribe<TMessage, TConsumer>(
			string queue,
			ushort prefetchCount = 0, 
			ushort consumersCount = 1) where TConsumer : MediaConsumerBase<TMessage>;

		/// <summary>
		/// Подписывает обработчик сообщений на очередь. Обработчик будет принимать только сообщения указанного типа.
		/// </summary>
		/// <typeparam name="TMessage">Тип сообщения.</typeparam>
		/// <typeparam name="TConsumer">Тип обработчика.</typeparam>
		/// <param name="queue">Название очереди, на которую нужно подписаться. Создает очередь, если её не было.</param>
		/// <param name="exchange">Название обменника, в который будут отсылаться сообщения. Создает обменник, если его не было.</param>
		/// <param name="prefetchCount">Количество одновременно обрабатываемых сообщений, если 0, то значение берется из настройки.</param>
		/// <param name="consumersCount">Количество параллельно подключаемых обработчиков.</param>
		/// <returns>Брокер сообщений <see cref="IMessageBroker"/>.</returns>
		IMessageBroker Subscribe<TMessage, TConsumer>(
			string queue,
			string exchange,
			ushort prefetchCount = 0,
			ushort consumersCount = 1) where TConsumer : MediaConsumerBase<TMessage>;

		/// <summary>
		/// Подписывает обработчик сообщений на очередь. Обработчик будет принимать только сообщения указанного типа.
		/// </summary>
		/// <typeparam name="TMessage">Тип сообщения.</typeparam>
		/// <typeparam name="TConsumer">Тип обработчика.</typeparam>
		/// <param name="queue">Название очереди, на которую нужно подписаться. Создает очередь, если её не было.</param>
		/// <param name="exchange">Название обменника, в который будут отсылаться сообщения. Создает обменник, если его не было.</param>
		/// <param name="routingKey">Ключ маршрутизации. Привязывает очередь к обменнику с помощью этого ключа. Если не пустой, то обменник создается с типом direct.</param>
		/// <param name="prefetchCount">Количество одновременно обрабатываемых сообщений, если 0, то значение берется из настройки.</param>
		/// <param name="consumersCount">Количество параллельно подключаемых обработчиков.</param>
		/// <returns>Брокер сообщений <see cref="IMessageBroker"/>.</returns>
		IMessageBroker Subscribe<TMessage, TConsumer>(
			string queue, 
			string exchange,
			string routingKey,
			ushort prefetchCount = 0,
			ushort consumersCount = 1) where TConsumer : MediaConsumerBase<TMessage>;

		Uri Host { get; }
	}
}
