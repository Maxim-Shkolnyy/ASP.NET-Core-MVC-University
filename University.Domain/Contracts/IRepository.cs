using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace University.Domain.Contracts;

public interface IRepository<T>
{
    IEnumerable<T> GetAll();
    T FindById(int id);
    IEnumerable<T> GetFiltered(Expression<Func<T, bool>> filter);
    IEnumerable<T> GetPaged(int skip, int take);
    IEnumerable<T> GetFilteredAndPaged(Expression<Func<T, bool>> filter, int skip, int take);
    int Count();
    int Count(Expression<Func<T, bool>> filter);
    T Update(T entity);
    T Add(T entity);
    bool DeleteById(int id);
}
