describe('item list spec', () => {
    it('displays items', () => {
        // arrange
        cy.intercept('/api/v1/items', (request) => {
            request.reply({ totalItems: 10, pageNumber: 1, pageSize: 5, items: [
                { itemId: '1ed35ab5-0055-4da2-b6fc-8721d136baba', name: 'Pappy Van Winkle 1 Year', sku: 'pappy-1', unitPrice: 991.99, imageUrl: 'https://www.oldripvanwinkle.com/app/uploads/2016/09/bourbon-bg-10year-2.jpg' },
                { itemId: '1ed35ab5-0055-4da2-b6fc-8721d136babb', name: 'Pappy Van Winkle 2 Year', sku: 'pappy-2', unitPrice: 992.99, imageUrl: 'https://www.oldripvanwinkle.com/app/uploads/2016/09/bourbon-bg-10year-2.jpg' },
                { itemId: '1ed35ab5-0055-4da2-b6fc-8721d136babc', name: 'Pappy Van Winkle 3 Year', sku: 'pappy-3', unitPrice: 993.99, imageUrl: 'https://www.oldripvanwinkle.com/app/uploads/2016/09/bourbon-bg-10year-2.jpg' },
                { itemId: '1ed35ab5-0055-4da2-b6fc-8721d136babd', name: 'Pappy Van Winkle 4 Year', sku: 'pappy-4', unitPrice: 994.99, imageUrl: 'https://www.oldripvanwinkle.com/app/uploads/2016/09/bourbon-bg-10year-2.jpg' },
                { itemId: '1ed35ab5-0055-4da2-b6fc-8721d136babe', name: 'Pappy Van Winkle 5 Year', sku: 'pappy-5', unitPrice: 995.99, imageUrl: 'https://www.oldripvanwinkle.com/app/uploads/2016/09/bourbon-bg-10year-2.jpg' },
            ] });
        });

        // act
        cy.visit('/');

        // assert
        cy.get('app-item-card')
            .should('have.length', 5)
            .first().should('contain.text', 'Pappy Van Winkle 1 Year')
            .should('contain.text', '$991.99');
    });
});
